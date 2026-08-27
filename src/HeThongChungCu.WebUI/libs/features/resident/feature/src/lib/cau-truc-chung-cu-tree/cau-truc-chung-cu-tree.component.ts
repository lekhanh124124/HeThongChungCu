import { FlatTreeControl } from '@angular/cdk/tree';
import { SelectionModel } from '@angular/cdk/collections';
import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NzTreeFlatDataSource, NzTreeFlattener } from 'ng-zorro-antd/tree-view';
import { ChungCuService, CauTrucToaNha, CauTrucTang, CauTrucCanHo } from '@features/resident/data-access';

type NodeType = 'root' | 'building' | 'floor' | 'apartment';

interface TreeNode {
  name: string;
  type: NodeType;
  buildingId?: number;
  tangId?: number;
  canHoId?: number;
  disabled?: boolean;
  children?: TreeNode[];
}

interface FlatNode {
  expandable: boolean;
  name: string;
  level: number;
  disabled: boolean;
  type: NodeType;
  buildingId?: number;
  tangId?: number;
  canHoId?: number;
}

@Component({
  selector: 'app-cau-truc-chung-cu-tree',
  templateUrl: './cau-truc-chung-cu-tree.component.html',
  styleUrls: ['./cau-truc-chung-cu-tree.component.scss']
})
export class CauTrucChungCuTreeComponent implements OnInit {
  keyword = '';
  private transformer = (node: TreeNode, level: number): FlatNode => ({
    expandable: !!node.children && node.children.length > 0,
    name: node.name,
    level,
    disabled: !!node.disabled,
    type: node.type,
    buildingId: node.buildingId,
    tangId: node.tangId,
    canHoId: node.canHoId
  });

  selectListSelection = new SelectionModel<FlatNode>(false);

  treeControl = new FlatTreeControl<FlatNode>(
    node => node.level,
    node => node.expandable
  );

  treeFlattener = new NzTreeFlattener<TreeNode, FlatNode>(
    this.transformer,
    node => node.level,
    node => node.expandable,
    node => node.children
  );

  dataSource = new NzTreeFlatDataSource(this.treeControl, this.treeFlattener);

  constructor(private svc: ChungCuService, private router: Router, private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.loadTree();
    this.svc.refresh$.subscribe(() => this.loadTree(this.keyword));
  }

  search(): void {
    this.loadTree(this.keyword);
  }

  clear(): void {
    this.keyword = '';
    this.loadTree();
  }

  private loadTree(keyword?: string): void {
    this.svc.getCauTrucChungCu(keyword).subscribe(r => {
      if (!r || !r.isOk || !Array.isArray(r.result)) {
        this.dataSource.setData([]);
        return;
      }

      const nodes = this.mapToTreeNodes(r.result || []);
      this.dataSource.setData(nodes);
      this.treeControl.expandAll();
    });
  }

  private mapToTreeNodes(data: CauTrucToaNha[]): TreeNode[] {
    const root: TreeNode = { name: 'Chung cư', type: 'root', children: [] };

    for (const building of data) {
      const buildingName = building.tenToaNha || building.maToaNha || `Tòa nhà #${building.id}`;
      const buildingNode: TreeNode = {
        name: buildingName,
        type: 'building',
        buildingId: building.id,
        children: []
      };

      const floors = building.cauTrucTangs || [];
      for (const floor of floors) {
        const floorName = floor.tenTang || floor.maTang || `Tầng #${floor.id}`;
        const floorNode: TreeNode = {
          name: floorName,
          type: 'floor',
          buildingId: building.id,
          tangId: floor.id,
          children: []
        };

        const apartments = floor.cauTrucCanHos || [];
        for (const apt of apartments) {
          const aptName = apt.tenCanHo || apt.maCanHo || `Căn hộ #${apt.id}`;
          floorNode.children!.push({
            name: aptName,
            type: 'apartment',
            buildingId: building.id,
            tangId: floor.id,
            canHoId: apt.id
          });
        }

        buildingNode.children!.push(floorNode);
      }

      root.children!.push(buildingNode);
    }

    return [root];
  }

  hasChild = (_: number, node: FlatNode): boolean => node.expandable;

  onNodeClick(node: FlatNode): void {
    this.selectListSelection.select(node);
    const currentUrl = this.router.url || '';

    // Khi đang ở màn quản lý quan hệ cư trú hoặc quản lý phương tiện, dùng cây như bộ lọc
    if (
      currentUrl.startsWith('/dashboard/resident/quan-he-cu-tru') ||
      currentUrl.startsWith('/dashboard/resident/phuong-tien') ||
      currentUrl.startsWith('/dashboard/resident/yeu-cau-cu-tru') ||
      currentUrl.startsWith('/dashboard/resident/requests') ||
      currentUrl.startsWith('/dashboard/dich-vu/chi-so-tieu-thu')
    ) {
      if (node.type === 'root') {
        this.router.navigate([], {
          relativeTo: this.route,
          queryParams: { filterScope: 'root' }
        });
      } else if (node.type === 'building' && node.buildingId != null) {
        this.router.navigate([], {
          relativeTo: this.route,
          queryParams: {
            filterScope: 'building',
            toaNhaId: node.buildingId,
            toaNhaName: node.name
          }
        });
      } else if (node.type === 'floor' && node.tangId != null) {
        this.router.navigate([], {
          relativeTo: this.route,
          queryParams: {
            filterScope: 'floor',
            toaNhaId: node.buildingId,
            tangId: node.tangId,
            tangName: node.name
          }
        });
      } else if (node.type === 'apartment' && node.canHoId != null) {
        this.router.navigate([], {
          relativeTo: this.route,
          queryParams: {
            filterScope: 'apartment',
            toaNhaId: node.buildingId,
            tangId: node.tangId,
            canHoId: node.canHoId
          }
        });
      }

      return;
    }

    // Mặc định: điều hướng như cũ cho màn quản lý chung cư
    if (node.type === 'root') {
      this.router.navigate(['toa-nha'], { relativeTo: this.route });
    } else if (node.type === 'building' && node.buildingId != null) {
      this.router.navigate(['tang'], {
        relativeTo: this.route,
        queryParams: { toaNhaId: node.buildingId, toaNhaName: node.name }
      });
    } else if (node.type === 'floor' && node.tangId != null) {
      this.router.navigate(['can-ho'], {
        relativeTo: this.route,
        queryParams: { tangId: node.tangId, tangName: node.name }
      });
    } else if (node.type === 'apartment' && node.canHoId != null) {
      this.router.navigate(['can-ho', node.canHoId], { relativeTo: this.route });
    }
  }
}
